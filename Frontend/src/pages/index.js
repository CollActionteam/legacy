import React from "react";
import Layout from "../components/Layout";
import { graphql } from "gatsby";
import { Banner } from "../components/Banner";
import { Container } from "@material-ui/core";
import { CallToAction } from "./home/call-to-action";

export const query = graphql`
query HomePageQuery {
  site {
    siteMetadata {
      title
    }
  }
  photos: allHomeYaml(filter: {name: {eq: "photos"}}) {
    edges {
      node {
        bannertitle
        bannerphoto
        name
      }
    }
  }
  content: allMarkdownRemark(filter: {frontmatter: {type: {eq: "home"}}}) {
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
}`;

export default ({ data }) => {
  const photos = data.photos.edges
    .map(e => e.node)
    .find(n => n.name = "photos");

  
    
  return (
    <Layout>
      <Banner photo={ photos.bannerphoto }>
        <CallToAction title={ photos.bannertitle }></CallToAction>
      </Banner>
      <Container>
        <p>Hello peoples</p>
      </Container>
    </Layout>
  );
}
