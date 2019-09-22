import React from "react";
import Layout from "../components/Layout";
import { Banner } from "../components/Banner";

import { graphql } from "gatsby";
import { CallToAction } from "../components/CallToAction";
import { Container, Grid } from "@material-ui/core";

import styles from "./index.module.scss";
import { CrowdactingSteps } from "../components/CrowdactingSteps";

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
      filter: { frontmatter: { type: { eq: "home" } } }) {
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
      <Grid className={ styles.banner }>
        <Banner photo={ photos.bannerphoto }>
          <Grid container className={ styles.dots }>
            <Container>
              <CallToAction title={ photos.bannertitle }></CallToAction>
            </Container>
          </Grid>
        </Banner>
      </Grid>
      <Grid className={ styles.introduction }>
        <Container>
            <h2>{ intro.frontmatter.title }</h2>
            <p dangerouslySetInnerHTML={{ __html: intro.html }}></p>
        </Container>
      </Grid>
      <Grid className={ styles.crowdactingsteps }>
        <Container>
          <CrowdactingSteps></CrowdactingSteps>
        </Container>
      </Grid>
    </Layout>
  );
}

export default Index;
