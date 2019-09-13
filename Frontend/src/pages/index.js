import React from "react";
import Layout from "../components/Layout";
import { Banner } from "../components/Banner";

import { graphql } from "gatsby";
import { CallToAction } from "../components/CallToAction";
import { Grid, Container } from "@material-ui/core";

import styles from "./index.module.scss";
import { ThreeSteps } from "../components/ThreeSteps";

export const query = graphql`
  query {
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

const Index = ({ data} ) => {
  const photos = data.photos.edges
    .map(e => e.node)
    .find(n => (n.name = "photos"));

  const sections = data.content.edges.map(e => e.node);
  const intro = sections.find(s => s.frontmatter.name === "intro");

  return (
    <Layout>
      <div className={ styles.main }>
        <Banner photo={ photos.bannerphoto }>
          <Container className={ styles.bannerContent }>
            <CallToAction title={ photos.bannertitle }></CallToAction>
          </Container>
        </Banner>
        <Container className={ styles.intro }>
          <Grid item xs={12}>
            <h2>{ intro.frontmatter.title }</h2>
          </Grid>
          <Grid item xs={12}>
            <p dangerouslySetInnerHTML={{ __html: intro.html }}></p>
          </Grid>
        </Container>
        <div className={ styles.threeSteps }>
          <Container>
            <ThreeSteps></ThreeSteps>
          </Container>
        </div>
      </div>
    </Layout>
  );
}

export default Index;
