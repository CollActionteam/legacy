import React from "react";
import Layout from "../../components/Layout";
import { graphql } from "gatsby";
import { Banner } from "../../components/Banner";
import styles from "./start.module.scss";
import { Section } from "../../components/Section";
import { StartProjectSteps } from "../../components/StartProjectSteps";
import { Button } from "../../components/Button";

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
        <div className={styles.banner}></div>>
      </Banner>
      <Section color="grey" className={styles.howItWorks}>
        <h1>How it works</h1>
        <StartProjectSteps></StartProjectSteps>
        <Button to="/projects/start">Start a project</Button>
      </Section>
    </Layout>
  );
};

export default StartProject;
