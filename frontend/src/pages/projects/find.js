import React from "react";
import { graphql } from "gatsby";
import Layout from "../../components/Layout";

export default ({ data }) => (
    <Layout>
        <h1>Find Project</h1>
        <p>This will be the find project page.</p>
    </Layout>
);

export const query = graphql`
  query {
    site {
      id
    }
  }
`;