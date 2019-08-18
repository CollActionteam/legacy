import React from "react";
import Layout from "../components/Layout";
import ProjectsList from "../components/ProjectsList";
import { graphql } from "gatsby";

export const query = graphql`
  query {
    site {
      siteMetadata {
        title
      }
    }
  }
`;

export default ({ data }) => (
  <Layout>
    <h1>Homepage</h1>
    <p>This will be the homepage.</p>
    <h2>{ data.site.siteMetadata.title }</h2>
    <ProjectsList></ProjectsList>
  </Layout>
);
