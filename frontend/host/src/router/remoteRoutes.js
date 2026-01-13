export const listenRemoteRoutes = (router) => {
  document.addEventListener("remoteNavigateTo", ((event) => {
    router.push(event.detail);
  }));
};