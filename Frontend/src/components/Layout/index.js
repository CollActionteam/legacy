import React from "react";
import Helmet from "react-helmet";
import { graphql, StaticQuery } from "gatsby";

// Styling
import "normalize.css";
import "./style.scss";

// Layout components
import Header from "../Header";
import Footer from "../Footer";

// Material-UI
import Grid from "@material-ui/core/Grid";

// FontAwesome
import { library } from "@fortawesome/fontawesome-svg-core";
import { fab } from "@fortawesome/free-brands-svg-icons";
import { faHeart } from "@fortawesome/free-solid-svg-icons";

library.add(fab, faHeart);

export default ({ children }) => (
  <StaticQuery
    query={graphql`
      query SiteTitleQuery {
        site {
          siteMetadata {
            title
            menuLinks {
              name
              link
            }
          }
        }
      }
    `}
    render={data => (
      <React.Fragment>
        <Helmet
          title={data.site.siteMetadata.title}
          meta={[
            { name: "description", content: "CollAction" },
            { name: "keywords", content: "collaction" },
          ]}
        ></Helmet>
        <Grid container className="site">
          <Header />
          <Grid container className="site-content">
            <Grid item xs={12}>
              {children}
            </Grid>
          </Grid>
          <Footer></Footer>
        </Grid>
      </React.Fragment>
    )}
  />
);
