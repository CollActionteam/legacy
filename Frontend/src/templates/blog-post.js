import React from "react";
import { graphql } from "gatsby";
import Layout from "../components/Layout";

export default function Template({ data }) {
  const { markdownRemark: post } = data;

  return (
    <Layout>
      <h1>{ post.frontmatter.title }</h1>
      <p dangerouslySetInnerHTML={{ __html: post.html }}></p>
    </Layout>
  )
}

export const pageQuery = graphql`
  query BlogPostByPath($slug: String!) {
    markdownRemark(fields: { slug: { eq: $slug } }) {
      html
      frontmatter {
        date(formatString: "MMMM DD, YYYY")
        title
      }
    }
  }
`