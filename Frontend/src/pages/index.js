import React from "react";
import Layout from "../components/Layout";
import { graphql } from "gatsby";
import { Intro } from "./home/intro";
import { HomepageBanner } from "./home/homepage-banner";

export const query = graphql`
  query HomePageQuery {
    site {
      siteMetadata {
        title
      }
    }
    photos: allHomeYaml(filter: { name: { eq: "photos" } }) {
      edges {
        node {
          bannertitle
          bannerphoto
          name
        }
      }
    }
    content: allMarkdownRemark(
      filter: { frontmatter: { type: { eq: "home" } } }
    ) {
      edges {
        node {
          html
          frontmatter {
            title
            name
          }
        }
      }
    }
  }
`;

export default ({ data }) => {
  const photos = data.photos.edges
    .map(e => e.node)
    .find(n => (n.name = "photos"));

  const sections = data.content.edges.map(e => e.node);
  const intro = sections.find(s => s.frontmatter.name === "intro");

  return (
    <Layout>
      <HomepageBanner photo={ photos.bannerphoto } title={ photos.bannertitle }></HomepageBanner>
      <Intro title={intro.frontmatter.title} content={intro.html}></Intro> 
    </Layout>
  );
};
