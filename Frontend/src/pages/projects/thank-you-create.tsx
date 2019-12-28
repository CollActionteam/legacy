import React from "react";
import { graphql } from "gatsby";
import styles from "./thank-you-create.module.scss";
import Layout from "../../components/Layout";
import { Overlay } from "../../components/Overlay";
import { Section } from "../../components/Section";

export const query = graphql`
  query {
    site {
      siteMetadata {
        title
      }
    }
    photos: allThankyoucreateYaml(
      filter: { name: { eq: "thankyoucreatephotos" } }
    ) {
      edges {
        node {
          thankyoucreatephoto
          name
        }
      }
    }
  }
`;

const ThankYou = ({ data }) => {
  const name = data.name || "ProjectName";
  const photos = data.photos.edges
    .map(e => e.node)
    .find(n => (n.name = "photos"));
  return (
    <Layout>
      <Overlay photo={photos.thankyoucreatephoto} media="min-width: 600px">
        <Section className={styles.thankYouOverlay}>
          <h1>Awesome!</h1>
          <h2>Thank you for submitting {name}!</h2>
          The CollAction Team will review your project as soon as possible.
          <br />
          If it meets all the CollAction criteria we’ll publish the project on
          the website and will let you know, so you can start promoting it!
          <br />
          If we have any additional questions or comments, we’ll reach out to
          you by email.
          <br />
          Thanks!
        </Section>
      </Overlay>
    </Layout>
  );
};

export default ThankYou;
