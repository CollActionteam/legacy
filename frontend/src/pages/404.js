import React from "react";
import { graphql } from "gatsby";

import Layout from "../components/Layout";

export default () => (
  <Layout>
    <h1>Not Found</h1>
    <p>You just hit a route that doesn&#39;t exist... the sadness.</p>
  </Layout>
);

export const pageQuery = graphql`
  query {
    site {
      siteMetadata {
        title
      }
    }
  }
`;
