import React from "react";
import Layout from "../components/Layout";
import { graphql } from "gatsby";

export default function About({ data }) {
  const videos = data.videos.edges
    .map(e => e.node)
    .find(n => n.name === "videos");
  
  const generateVideo = (className) => {
    return (
      <div className={ className }>
        <a href={ videos.mainvideo }>{ videos.mainvideo }</a>
      </div>
    )
  }

  const sections = data.allMarkdownRemark.edges.map(e => e.node);
  const mission = sections.find(s => s.frontmatter.name === "mission");
  const about = sections.find(s => s.frontmatter.name === "about");
  const join = sections.find(s => s.frontmatter.name === "join");
  const partners = sections.find(s => s.frontmatter.name === "partners");

  const generateSection = (section, className) => {
    return (
      <div className={ className }>
        <h1>{ section.frontmatter.title }</h1>
        <div dangerouslySetInnerHTML={{ __html: section.html }}></div>
      </div>
    )
  }

  const teamSection = data.team.edges
    .map(e => e.node)
    .find(n => n.name === "team");    

  const generateTeamMembers = (className) => {
    return (
      <div className={ className }>
        <h1>{ teamSection.title }</h1>
        <ul>
          { teamSection.team.map(generateMemberPhoto) }
        </ul>
      </div>
    )
  }

  const generateMemberPhoto = (member) => {
    return (
      <li key={ member.name }>            
        <img src={ member.photo } alt={ member.name } title={ member.name} />
        <span>{ member.name }</span>
      </li>
    );
  }

  return (
    <Layout>
      <div>
        { generateVideo("white") }
        { generateSection(mission, "green") }
        { generateSection(about, "white") }
        { generateSection(join, "grey") }
        { generateTeamMembers("white")}
        { generateSection(partners, "grey") }
      </div>
    </Layout>  
  );
}

export const pageQuery = graphql`
  query AboutPageQuery {
    allMarkdownRemark(filter: {frontmatter: {type: {eq: "about"}}}) {
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
    team: allAboutYaml(filter: {name: {eq: "team"}}) {
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
    videos: allAboutYaml(filter: {name: {eq: "videos"}}) {
      edges {
        node {
          name
          mainvideo
        }
      }
    }
  }
`