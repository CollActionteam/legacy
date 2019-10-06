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
      <div className={styles.teamMember}>
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
          frameBorder="0"
          allowFullScreen
        ></iframe>
      </Grid>
      <Grid className={styles.green}>
        <Container className={styles.mission}>
          <span dangerouslySetInnerHTML={{ __html: mission.html }}></span>
        </Container>
      </Grid>
      <Grid>
        <Container className={styles.about}>
          <span dangerouslySetInnerHTML={{ __html: about.html }}></span>
        </Container>
      </Grid>
      <Grid className={styles.grey}>
        <Container className={styles.team}>
          <h2>{meetTheTeam.title}</h2>
          <ul>{meetTheTeam.team.map(generateMemberPhoto)}</ul>
        </Container>
      </Grid>
      <Grid>
        <Container className={styles.join}>
          <span dangerouslySetInnerHTML={{ __html: join.html }}></span>
        </Container>
      </Grid>
      <Grid className={styles.grey}>
        <Container className={styles.partners}>
          <span dangerouslySetInnerHTML={{ __html: partners.html }}></span>
        </Container>
      </Grid>
      <Grid>
        <Container className={styles.faq}>
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
