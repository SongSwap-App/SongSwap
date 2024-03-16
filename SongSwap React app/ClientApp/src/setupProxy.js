const { createProxyMiddleware } = require('http-proxy-middleware');
const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}/api` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'https://localhost:5000/api';


module.exports = function(app) {
  const appProxy = createProxyMiddleware({
    proxyTimeout: 10000,
      target: target,
      secure: true,
    headers: {
      Connection: 'Keep-Alive'
    }
  });

  app.use('/api', appProxy);
};
