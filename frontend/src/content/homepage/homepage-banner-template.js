import React from "react";
import { graphql } from "gatsby";

class HomepageBannerTemplate extends React.Component {
  render() {
    const post = this.props.data.markdownRemark;

    return (
      <div>
        <h1>A custom banner</h1>
        <div dangerouslySetInnerHTML={{ __html: post.html }} />
      </div>
    );
  }
}

export default HomepageBannerTemplate;

export const pageQuery = graphql`
  query HomepageBanner() {
    markdownRemark() {
      html
    }
  }
`;
