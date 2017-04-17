
import * as React from "react";
import { ShareButtons as ReactShareButtons } from "react-share";
import renderComponentIf from "./renderComponentIf";

const {
  FacebookShareButton,
  TwitterShareButton,
  LinkedinShareButton,
  InstagramShareButton,
  YoutubeShareButton,
} = ReactShareButtons;

interface IShareButtonsProps {
  title: string;
  url: string;
}

class ShareButtons extends React.Component<IShareButtonsProps, null> {
  render () {
    const url: string = window.location.toString();
    return (
      <div className="share-buttons">
        <FacebookShareButton url={this.props.url} title={this.props.title} >
          <i className="fa fa-facebook"></i>
        </FacebookShareButton>
        <TwitterShareButton url={this.props.url} title={this.props.title}>
          <i className="fa fa-twitter"></i>
        </TwitterShareButton>
        <LinkedinShareButton url={this.props.url} title={this.props.title}>
          <i className="fa fa-linkedin"></i>
        </LinkedinShareButton>
     </div>
    );
  }
}

renderComponentIf(
  <ShareButtons title="CollAction" url="collaction.com" />,
  document.getElementById("homepage-share-buttons")
);

renderComponentIf(
  <ShareButtons title="CollAction" url="collaction.com" />,
  document.getElementById("project-details-share-buttons")
);