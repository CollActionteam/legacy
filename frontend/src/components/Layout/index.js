import React from "react";
import Helmet from "react-helmet";
import Header from "../Header";
import Footer from "../Footer";
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
              { name: 'description', content: 'Sample' },
              { name: 'keywords', content: 'sample, something' },
            ]}
          >
          </Helmet>
          <Header menuLinks={data.site.siteMetadata.menuLinks} />
          <div style={{ padding: "1rem" }}>
            {children}
          </div>
          <Footer></Footer>
        </React.Fragment>
    )}
  />
)