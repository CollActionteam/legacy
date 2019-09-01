import React from "react";
import Layout from "../components/Layout";
import { graphql } from "gatsby";
import Section from "../components/Section";

export default function About({ data }) {
  const videos = data.videos.edges
    .map(e => e.node)
    .find(n => n.name === "videos");

  const sections = data.allMarkdownRemark.edges.map(e => e.node);
  const mission = sections.find(s => s.frontmatter.name === "mission");
  const about = sections.find(s => s.frontmatter.name === "about");
  const join = sections.find(s => s.frontmatter.name === "join");
  const partners = sections.find(s => s.frontmatter.name === "partners");

  const teamSection = data.team.edges
    .map(e => e.node)
    .find(n => n.name === "team");

  const generateVideo = className => {
    return (
      <div className={className}>
        <a href={videos.mainvideo}>{videos.mainvideo}</a>
      </div>
    );
  };

  const generateSection = (section, color) => (
    <Section color={color} title={section.frontmatter.title}>
      <div dangerouslySetInnerHTML={{ __html: section.html }}></div>
    </Section>
  );

  const generateTeamMembers = color => (
    <Section color={color} title={teamSection.title}>
      <ul>{teamSection.team.map(generateMemberPhoto)}</ul>
    </Section>
  );

  const generateMemberPhoto = member => (
    <li key={member.name}>
      <img src={member.photo} alt={member.name} title={member.name} />
      <span>{member.name}</span>
    </li>
  );

  return (
    <Layout>
      <div>
        {generateVideo("white")}
        {generateSection(mission, "green")}
        {generateSection(about)}
        {generateTeamMembers("grey")}
        {generateSection(join)}
        {generateSection(partners, "grey")}
      </div>
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
