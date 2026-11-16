using './main.bicep'

param environmentName = 'prod'
param image = readEnvironmentVariable('IMAGE', 'mcr.microsoft.com/k8se/quickstart:latest')
param targetPort = int(readEnvironmentVariable('TARGET_PORT', '80'))

// Custom domain. Needs the A and asuid TXT records at the DNS provider (see main.bicep).
param customDomain = 'glennhostetler.com'

// Step 1 of 2: false adds the hostname unbound so Azure can issue the free certificate.
// Step 2 of 2: change to true once the certificate shows Succeeded, to bind it.
param bindCustomDomain = false
