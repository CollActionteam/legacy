import React from "react";
import Helmet from "react-helmet";
import Header from "../Header";
import Footer from "../Footer";
import { graphql, StaticQuery } from "gatsby";
import { Container, Row, Col } from 'reactstrap';

//Styling
import "bootstrap/dist/css/bootstrap.min.css";
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
        <Container>
          <Header menuLinks={data.site.siteMetadata.menuLinks} />
          <Row>
            <Col>
              <div style={{ padding: "1rem" }}>{children}</div>
            </Col>
          </Row>
          <Footer></Footer>
        </Container>
      </React.Fragment>
    )}
  />
);
