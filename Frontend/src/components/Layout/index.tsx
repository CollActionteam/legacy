import React from "react";
import Helmet from "react-helmet";

// Styling
import "normalize.css";
import "./style.scss";

// Layout components
import Header from "../Header/Header";
import Footer from "../Footer/Footer";

// Material-UI
import Grid from "@material-ui/core/Grid";

// FontAwesome
import { library } from "@fortawesome/fontawesome-svg-core";
import { fab } from "@fortawesome/free-brands-svg-icons";
import {
  faHeart,
  faTimes,
  faBars,
  faUser,
  faAngleDown,
  faSpinner,
  faClock,
  faTools,
  faExclamationCircle
} from "@fortawesome/free-solid-svg-icons";
import { faEnvelope } from "@fortawesome/free-regular-svg-icons";

library.add(
  fab,
  faHeart,
  faEnvelope,
  faTimes,
  faBars,
  faAngleDown,
  faSpinner,
  faClock,
  faUser,
  faTools,
  faExclamationCircle
);

export default ({ children }: any) => (
  <React.Fragment>
    <Helmet
      title="CollAction"
      meta={[
        { name: "description", content: "CollAction" },
        { name: "keywords", content: "collaction" },
      ]}
    ></Helmet>
    <Header />
    <Grid container className="site-content">
      <Grid item xs={12}>
        {children}
      </Grid>
    </Grid>
    <Footer></Footer>
  </React.Fragment>);

