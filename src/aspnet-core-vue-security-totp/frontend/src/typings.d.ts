// Ignore missing vendor definitions
// https://github.com/Microsoft/TypeScript/issues/3691

// TypeScript support?
// https://github.com/gera2ld/qrcanvas-vue/issues/7
// declare module "qrcanvas-vue" {
//   let Qrcanvas: any;
//   export = Qrcanvas;
// }

declare module "nprogress/nprogress" {
  let NProgress: any;
  export = NProgress;
}

declare module "vue-loading-overlay" {
  let Loading: any;
  export = Loading;
}
