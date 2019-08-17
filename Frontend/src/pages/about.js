import React from "react";
import Layout from "../components/Layout";
import { graphql } from "gatsby";

export default function About({ data }) {
  const sections = data.allMarkdownRemark.edges.map(e => e.node);

  const mission = sections.find(s => s.frontmatter.name === "mission");
  const about = sections.find(s => s.frontmatter.name === "about");
  const join = sections.find(s => s.frontmatter.name === "join");
  const partners = sections.find(s => s.frontmatter.name === "partners");

  const generateSection = (section, className) => {
    return (
      <div class={ className }>
        <h1>{ section.frontmatter.title }</h1>
        <div dangerouslySetInnerHTML={{ __html: section.html }}></div>
      </div>
    )
  }

  const generateMemberPhoto = (member) => {
    return (
      <div>            
        <img src={ member.photo } alt={ member.name } title={ member.name} />
        <span>{ member.name }</span>
      </div>
    );
  }

  const team = data.aboutYaml;
  const generateTeamMembers = (className) => {
    return (
      <div class={ className }>
        <h1>{ team.title }</h1>
        { team.team.map(generateMemberPhoto) }
      </div>
    )
  }

  return (
    <Layout>
      <div>
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
    aboutYaml {
      title
      team {
        name
        photo
      }
    }  
  }
`