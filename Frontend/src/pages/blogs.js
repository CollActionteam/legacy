import React from "react";
import Layout from "../components/Layout";
import { Link, graphql } from "gatsby";

export default function Blogs({ data }) {
  const { edges: posts } = data.allMarkdownRemark;

  return (
    <Layout>
      <h1>Here be blogs</h1>
      <p>Available blogs:</p>
      <ul>
        { posts
          .filter(post => post.node.frontmatter.title.length > 0)
          .map(( { node: post }) => {
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
    allMarkdownRemark(sort: { order: DESC, fields: [frontmatter___date] }) {
      edges {
        node {
          excerpt(pruneLength: 100)
          id
          frontmatter {
            title
            date(formatString: "MMMM DD, YYYY")
          }
          fields {
            slug
          }
        }
      }
    }
  }
`