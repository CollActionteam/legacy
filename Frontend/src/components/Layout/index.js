import React from "react";
import Helmet from "react-helmet";
import Header from "../Header";
import Footer from "../Footer";

// Material-UI
import { ThemeProvider } from "@material-ui/styles";
import CssBaseline from '@material-ui/core/CssBaseline';
import Container from "@material-ui/core/Container";
import theme from "../../theme";

import { graphql, StaticQuery } from "gatsby";

//Styling
import "./style.scss";

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
          <Container>
            <Header />
            <div style={{ padding: "1rem" }}>{children}</div>
            <Footer></Footer>
          </Container>
        </ThemeProvider>
      </React.Fragment>
    )}
  />
);
