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
import { ThemeProvider } from "@material-ui/styles";
import CssBaseline from '@material-ui/core/CssBaseline';
import Container from "@material-ui/core/Container";
import Grid from "@material-ui/core/Grid";
import theme from "../../theme";

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
        <ThemeProvider theme={theme}>
          <CssBaseline />
          <Header />
          <Container>
            <Grid container>
              <Grid item xs={12}>
                {children}
              </Grid>
            </Grid>
          </Container>
          <Footer></Footer>
        </ThemeProvider>
      </React.Fragment>
    )}
  />
);
