import React from "react";
import Router from "./routes";
import ThemeConfig from "./theme";
import ScrollToTop from "./components/ScrollToTop";

export default function App() {
  return (
    <ThemeConfig>
      <ScrollToTop />
      <Router />
    </ThemeConfig>
  );
}
