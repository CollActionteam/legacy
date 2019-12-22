import React from "react";
import Layout from "../../components/Layout";
import { graphql } from "gatsby";
import { Banner } from "../../components/Banner";
import styles from "./start.module.scss";
import { Section } from "../../components/Section";
import { StartProjectSteps } from "../../components/StartProjectSteps";
import { Button, SecondaryButton } from "../../components/Button/Button";
import { Faq } from "../../components/Faq";
import { Grid } from "@material-ui/core";

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
    faqs: allMarkdownRemark(
      filter: { frontmatter: { type: { eq: "startproject_faq" } } }
      sort: { fields: frontmatter___sequence }
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
    .find(n => (n.name = "photos"));

  const intro = data.content.edges
    .map(e => e.node)
    .find(n => n.frontmatter.name === "intro");

  const faqs = data.faqs.edges.map(e => e.node);

  return (
    <Layout>
      <Banner photo={photos.startprojectphoto}>
        <div className={styles.banner}></div>
      </Banner>
      <Section className={styles.introBlock}>
        <span dangerouslySetInnerHTML={{ __html: intro.html }}></span>
        <div className={styles.cta}>
          <SecondaryButton to="/projects/create">
            Start a project
          </SecondaryButton>
        </div>
      </Section>

      <Section color="grey" className={styles.howItWorks} title="How it works">
        <StartProjectSteps></StartProjectSteps>
        <Button to="/projects/create">Start a project</Button>
      </Section>
      <Grid container>
        <Grid item md={2}></Grid>
        <Grid item xs={12} md={8}>
          <Section className={styles.faq} title="Frequently Asked Questions">
            {faqs.map(faq => (
              <Faq
                key={faq.frontmatter.name}
                title={faq.frontmatter.name}
                content={faq.html}
              ></Faq>
            ))}
          </Section>
        </Grid>
      </Grid>
    </Layout>
  );
};

export default StartProject;
