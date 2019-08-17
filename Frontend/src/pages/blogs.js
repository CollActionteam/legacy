import React from "react";
import Layout from "../components/Layout";
import { Link, graphql } from "gatsby";

export default function Blogs({ data }) {
  const posts = data.allMarkdownRemark.edges;

  return (
    <Layout>
      <h1>Here be blogs</h1>
      <p>Available blogs:</p>
      <ul>
        { 
          posts
            .map(e => e.node)
            .map(p => {
              return (
                <li key={ p.fields.slug }>
                  <Link to={ `${p.frontmatter.type}/${p.fields.slug}` }>{ p.frontmatter.title }</Link><br/>
                  <span>{ p.excerpt }</span>
                </li>
              )
            })
        }
      </ul>
    </Layout>
  );
}

export const pageQuery = graphql`
  query BlogsQuery {
    allMarkdownRemark(filter: {frontmatter: {type: {eq: "blogs"}}}, sort: {order: DESC, fields: frontmatter___date}) {
      edges {
        node {
          frontmatter {
            type
            title
          }
          fields {
            slug
          }
          excerpt(pruneLength: 100)
        }
      }
    }
  }
`