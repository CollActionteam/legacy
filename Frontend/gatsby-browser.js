import React from "react";
import "core-js/modules/es6.set";
import "core-js/modules/es6.map";
import "raf/polyfill";
import Apollo from "./src/providers/apollo";

import "slick-carousel/slick/slick.css";
import "slick-carousel/slick/slick-theme.css";

export const wrapRootElement = ({ element }) => <Apollo>{element}</Apollo>;
