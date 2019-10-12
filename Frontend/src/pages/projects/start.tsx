import React from "react";
import Layout from "../../components/Layout";
import { graphql } from "gatsby";
import { Banner } from "../../components/Banner";

export const query = graphql`
  query {
    site {
      siteMetadata {
        title
      }
    }
    photos: allStartprojectYaml(
      filter: { name: { eq: "startprojectphotos" } }
    ) {
      edges {
        node {
          startprojectphoto
          name
        }
      }
    }
    content: allMarkdownRemark(
      filter: { frontmatter: { type: { eq: "startproject" } } }
    ) {
      edges {
        node {
          html
          frontmatter {
            name
          }
        }
      }
    }
  }
`;

const StartProject = ({ data }) => {
  const photos = data.photos.edges
    .map(e => e.node)
    .find(n => (n.name = "startprojectphotos"));

  return (
    <Layout>
      <Banner photo={photos.startprojectphoto}>
        Hello
      </Banner>
    </Layout>
  );
};

export default StartProject;
