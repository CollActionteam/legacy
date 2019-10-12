import React from "react";
import Layout from "../components/Layout";
import { graphql } from "gatsby";
import { Grid, Container } from "@material-ui/core";

import styles from "./about.module.scss";
import { Section } from "../components/Section";
import { Faq } from "../components/Faq";

export default function About({ data }) {
  const videos = data.videos.edges
    .map(e => e.node)
    .find(n => n.name === "videos");

  const sections = data.sections.edges.map(e => e.node);
  const mission = sections.find(s => s.frontmatter.name === "mission");
  const about = sections.find(s => s.frontmatter.name === "about");
  const join = sections.find(s => s.frontmatter.name === "join");
  const partners = sections.find(s => s.frontmatter.name === "partners");

  const meetTheTeam = data.team.edges
    .map(e => e.node)
    .find(n => n.name === "team");

  const faqs = data.faqs.edges.map(e => e.node);

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
      <Section indent color="green">
        <span dangerouslySetInnerHTML={{ __html: mission.html }}></span>
      </Section>
      <Section indent>
        <span dangerouslySetInnerHTML={{ __html: about.html }}></span>
      </Section>
      <Section indent color="grey">
        <h2>{meetTheTeam.title}</h2>
        <ul className={styles.team}>
          {meetTheTeam.team.map(generateMemberPhoto)}
        </ul>
      </Section>
      <Section indent>
        <span dangerouslySetInnerHTML={{ __html: join.html }}></span>
      </Section>
      <Section indent color="grey">
        <span dangerouslySetInnerHTML={{ __html: partners.html }}></span>
      </Section>
      <Section indent>
        <h2>Frequently Asked Questions</h2>
        {faqs.map(faq => (
          <Faq
            key={faq.frontmatter.name}
            title={faq.frontmatter.name}
            content={faq.html}
          ></Faq>
        ))}
      </Section>
    </Layout>
  );
}

export const pageQuery = graphql`
  query AboutPageQuery {
    sections: allMarkdownRemark(
      filter: { frontmatter: { type: { eq: "about" } } }
    ) {
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
    faqs: allMarkdownRemark(
      filter: { frontmatter: { type: { eq: "about_faq" } } }
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
