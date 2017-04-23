
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
  title?: string;
  url?: string;
}

class ShareButtons extends React.Component<IShareButtonsProps, null> {
  render () {
    const url: string = window.location.host + window.location.pathname;
    const title: string = this.props.title || window.document.title;
    return (
      <div className="share-buttons">
        <FacebookShareButton title={title} url={url} >
          <i className="fa fa-facebook"></i>
        </FacebookShareButton>
        <TwitterShareButton url={url} title={title}>
          <i className="fa fa-twitter"></i>
        </TwitterShareButton>
        <LinkedinShareButton url={url} title={title} description={url}>
          <i className="fa fa-linkedin"></i>
        </LinkedinShareButton>
     </div>
    );
  }
}

renderComponentIf(
  <ShareButtons />,
  document.getElementById("homepage-share-buttons")
);

renderComponentIf(
  <ShareButtons />,
  document.getElementById("project-details-share-buttons")
);