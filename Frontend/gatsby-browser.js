import React from "react";
import "core-js/modules/es6.set";
import "core-js/modules/es6.map";
import "raf/polyfill";
import Apollo from "./src/providers/apollo";

export const wrapRootElement = ({ element }) => <Apollo>{element}</Apollo>;