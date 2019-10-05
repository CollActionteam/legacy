import React from "react";
import Layout from "../components/Layout";
import { graphql } from "gatsby";
import { Grid, Container } from "@material-ui/core";

import styles from "./about.module.scss";

export default function About({ data }) {
  const videos = data.videos.edges
    .map(e => e.node)
    .find(n => n.name === "videos");

  const sections = data.allMarkdownRemark.edges.map(e => e.node);
  const mission = sections.find(s => s.frontmatter.name === "mission");
  const about = sections.find(s => s.frontmatter.name === "about");
  const join = sections.find(s => s.frontmatter.name === "join");
  const partners = sections.find(s => s.frontmatter.name === "partners");

  const meetTheTeam = data.team.edges
    .map(e => e.node)
    .find(n => n.name === "team");

  const generateMemberPhoto = member => (
    <li key={member.name}>
      <div>
        <img src={member.photo} alt={member.name} title={member.name} />
        <p>{member.name}</p>
      </div>
    </li>
  );

  return (
    <Layout>
      <Grid className={styles.video}>
        <iframe
          title="Collective actions"
          src={videos.mainvideo}
          frameborder="0"
          allowFullScreen
        ></iframe>
      </Grid>
      <Grid className={styles.mission}>
        <Container className={styles.missionContainer}>
          <span dangerouslySetInnerHTML={{ __html: mission.html }}></span>
        </Container>
      </Grid>
      <Grid className={styles.about}>
        <Container className={styles.aboutContainer}>
          <span dangerouslySetInnerHTML={{ __html: about.html }}></span>
        </Container>
      </Grid>
      <Grid className={styles.team}>
        <Container>
          <h2>{meetTheTeam.title}</h2>
          <ul>{meetTheTeam.team.map(generateMemberPhoto)}</ul>
        </Container>
      </Grid>
      <Grid className={styles.join}>
        <Container className={styles.joinContainer}>
          <span dangerouslySetInnerHTML={{ __html: join.html }}></span>
        </Container>
      </Grid>
      <Grid className={styles.partners}>
        <Container className={styles.partnersContainer}>
          <span dangerouslySetInnerHTML={{ __html: partners.html }}></span>
        </Container>
      </Grid>
      <Grid className={styles.faq}>
        <Container>
          <h2>Frequently Asked Questions</h2>
          <p>&lt;Loaded from the CMS...&gt;</p>
        </Container>
      </Grid>
    </Layout>
  );
}

export const pageQuery = graphql`
  query AboutPageQuery {
    allMarkdownRemark(filter: { frontmatter: { type: { eq: "about" } } }) {
      edges {
        node {
          html
          frontmatter {
            name
            title
          }
        }
      }
    }
    team: allAboutYaml(filter: { name: { eq: "team" } }) {
      edges {
        node {
          name
          title
          team {
            photo
            name
          }
        }
      }
    }
    videos: allAboutYaml(filter: { name: { eq: "videos" } }) {
      edges {
        node {
          name
          mainvideo
        }
      }
    }
  }
`;
