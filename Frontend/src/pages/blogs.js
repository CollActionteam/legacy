import React from "react";
import Layout from "../components/Layout";
import { Link, graphql } from "gatsby";

export default function Blogs({ data }) {
  const { edges: posts } = data.allFile;

  return (
    <Layout>
      <h1>Here be blogs</h1>
      <p>Available blogs:</p>
      <ul>
        { posts
          .filter(post => post.node.childMarkdownRemark !== null)
          .map(post => post.node.childMarkdownRemark)
          .map(post => {
            return (
              <li>
                <Link to={ post.fields.slug }>{ post.frontmatter.title }</Link><br/>
                <p>{ post.excerpt }</p>
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
    allFile(filter: {sourceInstanceName: {eq: "blogs"}}, sort: {fields: childMarkdownRemark___frontmatter___date, order: DESC}) {
      edges {
        node {
          childMarkdownRemark {
            frontmatter {
              title
              date(formatString: "MMMM DD, YYYY")
            }
            excerpt(pruneLength: 100)
            fields {
              slug
            }
          }
        }
      }
    }
  }
`