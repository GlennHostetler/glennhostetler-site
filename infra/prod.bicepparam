using './main.bicep'

param environmentName = 'prod'
param image = readEnvironmentVariable('IMAGE', 'mcr.microsoft.com/k8se/quickstart:latest')
param targetPort = int(readEnvironmentVariable('TARGET_PORT', '80'))

// Custom domain. Needs the A and asuid TXT records at the DNS provider (see main.bicep).
param customDomain = 'glennhostetler.com'

// Two steps, because the certificate can only be requested after the app has the hostname:
//   step 1: false = add the hostname unbound so Azure can issue the free certificate (done)
//   step 2: true  = bind the hostname to that certificate (certificate shows Succeeded)
// Leave this true from now on: with false, the next deploy would unbind the domain.
param bindCustomDomain = true
