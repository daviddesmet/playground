import Vue from "vue";
import VueCookies from "vue-cookies";
Vue.use(VueCookies);

const ID_TOKEN_KEY = "auth_token";

export const getToken = (id = ID_TOKEN_KEY) => {
  return Vue.$cookies.get(id || ID_TOKEN_KEY);
  // return window.localStorage.getItem(id || ID_TOKEN_KEY);
};

export const saveToken = (token: string, id = ID_TOKEN_KEY) => {
  // TODO: Add cookie expiration (can be obtained from auth service)
  Vue.$cookies.set(id || ID_TOKEN_KEY, token);
  // window.localStorage.setItem(id || ID_TOKEN_KEY, token);
};

export const destroyToken = (id = ID_TOKEN_KEY) => {
  return Vue.$cookies.remove(id || ID_TOKEN_KEY);
  // window.localStorage.removeItem(id || ID_TOKEN_KEY);
};

// source https://stackoverflow.com/questions/38552003/how-to-decode-jwt-token-in-javascript/38552302#38552302
export const parseToken = (token: string) => {
  const base64Url = token.split(".")[1];
  const base64 = base64Url.replace("-", "+").replace("_", "/");
  return JSON.parse(window.atob(base64));
};

export const getClaims = (id = ID_TOKEN_KEY) => {
  const token = getToken(id);
  if (!token) {
    return null;
  }

  return parseToken(token);
};

export default { getToken, saveToken, destroyToken, parseToken, getClaims };
