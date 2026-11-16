using './main.bicep'

param environmentName = 'dev'
param image = readEnvironmentVariable('IMAGE', 'mcr.microsoft.com/k8se/quickstart:latest')
param targetPort = int(readEnvironmentVariable('TARGET_PORT', '80'))
