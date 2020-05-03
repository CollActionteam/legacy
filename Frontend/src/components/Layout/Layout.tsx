import React from "react";
import { Helmet } from "react-helmet";

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
import { 
    faTwitter, 
    faFacebookF,
    faInstagram, 
    faYoutube, 
    faLinkedinIn
} from "@fortawesome/free-brands-svg-icons";
import {
    faTimes,
    faBars,
    faAngleDown,
    faSpinner,
    faClock,
    faTools,
    faExclamationCircle,
    faTrash,
    faEuroSign,
    faPlus,
    faMinus
} from "@fortawesome/free-solid-svg-icons";
import { faEnvelope, faHeart, faUser } from "@fortawesome/free-regular-svg-icons";
import CookieDialog from "../CookieDialog/CookieDialog";
import { useLocation } from "react-router-dom";

library.add(
  faHeart,
  faEnvelope,
  faTimes,
  faBars,
  faAngleDown,
  faSpinner,
  faClock,
  faUser,
  faTools,
  faExclamationCircle,
  faTrash,
  faEuroSign,
  faPlus,
  faMinus,
  faTwitter,
  faFacebookF,
  faInstagram,
  faYoutube,
  faLinkedinIn
);

export default ({ children }: any) => {
  const { pathname } = useLocation();
  const needsLayout = !pathname.endsWith("/widget");
  if (needsLayout) {
    return <>
      <Helmet
        defaultTitle="CollAction"
        titleTemplate="%s — CollAction"
        meta={[
          { name: "description", content: "CollAction" },
          { name: "keywords", content: "collaction" },
        ]}
      ></Helmet>
      <Header />
      <Grid container className="site-content">
        <Grid item xs={12}>
          <CookieDialog />
        </Grid>
        <Grid item xs={12}>
          {children}
        </Grid>
      </Grid>
      <Footer></Footer>
    </>;
  } else {
    return <>
      { children}
    </>;
  }
}; 