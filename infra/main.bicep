@description('Short name used in resource names')
param workload string = 'glennsite'

@description('Which environment this is: sandbox, dev or prod')
param environmentName string = 'sandbox'

param location string = resourceGroup().location

@description('Container image to run. Starts as a public sample, later our own image.')
param image string = 'mcr.microsoft.com/k8se/quickstart:latest'

@description('Port the container listens on. 80 for the sample, 8080 for our site.')
param targetPort int = 80

var suffix = uniqueString(resourceGroup().id)
var registryName = 'acr${workload}${suffix}'

// Built-in Azure role: "AcrPull" (can pull images, nothing else).
var acrPullRoleId = '7f951dda-4ed3-4680-a7ca-43fe172d538d'

// ---------- Registry ----------
resource registry 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: registryName
  location: location
  sku: {
    name: 'Basic'
  }
  properties: {
    adminUserEnabled: false
  }
}

// ---------- Logs ----------
resource logs 'Microsoft.OperationalInsights/workspaces@2023-09-01' = {
  name: 'log-${workload}-${environmentName}'
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
    retentionInDays: 30
  }
}

// ---------- Identity, and permission to pull from the registry ----------
resource identity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: 'id-${workload}-${environmentName}'
  location: location
}

resource acrPull 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  scope: registry
  name: guid(registry.id, identity.id, acrPullRoleId)
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', acrPullRoleId)
    principalId: identity.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

// ---------- Container Apps environment ----------
resource environment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: 'cae-${workload}-${environmentName}'
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logs.properties.customerId
        sharedKey: logs.listKeys().primarySharedKey
      }
    }
  }
}

// ---------- The app ----------
resource app 'Microsoft.App/containerApps@2024-03-01' = {
  name: 'ca-${workload}-${environmentName}'
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${identity.id}': {}
    }
  }
  properties: {
    managedEnvironmentId: environment.id
    configuration: {
      ingress: {
        external: true
        targetPort: targetPort
      }
      registries: [
        {
          server: registry.properties.loginServer
          identity: identity.id
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'web'
          image: image
          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
        }
      ]
      scale: {
        minReplicas: 0
        maxReplicas: 2
      }
    }
  }
  dependsOn: [
    acrPull
  ]
}

output registryName string = registry.name
output registryLoginServer string = registry.properties.loginServer
output appUrl string = 'https://${app.properties.configuration.ingress.fqdn}'
