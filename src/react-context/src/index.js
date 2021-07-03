import "simplebar/src/simplebar.css";

import ReactDOM from "react-dom";
import { BrowserRouter } from "react-router-dom";
import { HelmetProvider } from "react-helmet-async";

import ContextWrapper from "./ContextWrapper";

import App from "./App";

ReactDOM.render(
  <HelmetProvider>
    <BrowserRouter>
      <ContextWrapper>
        <App />
      </ContextWrapper>
    </BrowserRouter>
  </HelmetProvider>,
  document.getElementById("root")
);
