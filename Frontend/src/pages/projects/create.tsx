import React from "react";
import Layout from "../../components/Layout";
import { graphql } from "gatsby";
import styles from "./create.module.scss";
import { Section } from "../../components/Section";
import { Grid, Container } from "@material-ui/core";
import { Button } from "../../components/Button";

export const query = graphql`
  query {
    site {
      siteMetadata {
        title
      }
    }
  }
`;

const CreateProject = () => {
  return (
    <Layout>
      <div className={styles.projectBanner}>
        <div className={styles.uploadBanner}>
          <h3>Drop banner image here</h3>
          <span>Use pjg, png, gif or bmp. Max. 1MB</span>
        </div>
      </div>
      <div className={styles.projectInfo}>
        <Section className={styles.projectInfoBlock}>
          <h3>Project name</h3>
          <p>Input box</p>
          <h3>Category</h3>
          <p>Select box</p>
          <h3>Target</h3>
          <p>Input box</p>
          <h3>Proposal</h3>
          <p>Textarea here</p>
        </Section>
      </div>

      <Container>
        <Grid container>
          <Grid item xs={12} md={5}>
            <Section>
              <h3>Short description</h3>
              <p>Rich text here</p>
              <h3>Sign up opens</h3>
              <p>Input box</p>
              <h3>Sign up closes</h3>
              <p>Input box</p>
              <h3>Hashtags</h3>
              <p>Input box</p>
            </Section>
          </Grid>
          <Grid item xs={12} md={7}>
            <Section>
              <h3>Goal/impact</h3>
              <p>Rich text here</p>
              <h3>Other comments</h3>
              <p>Rich text here</p>
              <h3>Descriptive image</h3>
              <p>File upload component</p>
              <h3>YouTube Video Link</h3>
              <p>Input box</p>
            </Section>
          </Grid>
          <Grid item xs={12}>
            <Section className={styles.submitProject}>
              <Button type="submit">Submit</Button>
            </Section>
          </Grid>
        </Grid>
      </Container>
    </Layout>
  );
};

export default CreateProject;
