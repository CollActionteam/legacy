import React from "react";
import Layout from "../components/Layout";
import { graphql } from "gatsby";

export default function About({ data }) {
  const sections = data.allMarkdownRemark.edges.map(e => e.node);

  const mission = sections.find(s => s.frontmatter.name === "mission");
  const about = sections.find(s => s.frontmatter.name === "about");
  const join = sections.find(s => s.frontmatter.name === "join");
  const partners = sections.find(s => s.frontmatter.name === "partners");

  return (
    <Layout>
      <div>
        <div>
          <h1>{ mission.frontmatter.title }</h1>
          <div dangerouslySetInnerHTML={{ __html: mission.html }}></div>
        </div>
        <div>
          <h1>{ about.frontmatter.title }</h1>
          <div dangerouslySetInnerHTML={{ __html: about.html }}></div>
        </div>
        <div>
          <h1>{ join.frontmatter.title }</h1>
          <div dangerouslySetInnerHTML={{ __html: join.html }}></div>
        </div>
        <div>
          <h1>{ partners.frontmatter.title }</h1>
          <div dangerouslySetInnerHTML={{ __html: partners.html }}></div>
        </div>
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
  }
`