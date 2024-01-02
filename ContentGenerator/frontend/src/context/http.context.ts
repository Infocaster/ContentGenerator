import { createContext } from "@lit/context";
export const httpKey = "http";
export const httpContext = createContext<angular.IHttpService>(httpKey);