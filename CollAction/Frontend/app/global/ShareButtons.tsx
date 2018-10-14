import * as React from "react";
import * as ReactDOM from "react-dom";
import renderComponentIf from "./renderComponentIf";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { library } from "@fortawesome/fontawesome-svg-core";
import { faTwitter } from "@fortawesome/free-brands-svg-icons";
import { faFacebook } from "@fortawesome/free-brands-svg-icons";
import { faLinkedin } from "@fortawesome/free-brands-svg-icons";

library.add(faTwitter, faFacebook, faLinkedin);

enum States { ERROR, READY, LOADING }

interface IShareButtonsProps {
  title?: string;
  url?: string;
}

interface IShareButtonsState {
  shareCount?: number;
  shareCountError?: Error;
  shareCountState: States;
}

class ShareButtons extends React.Component<IShareButtonsProps, IShareButtonsState> {

  constructor(props) {
    super(props);
    this.state = { shareCountState: States.LOADING };
  }

  componentDidMount() {
    this.getShareCount();
  }

  getTitle() {
    if (this.props.title)
      return encodeURIComponent(this.props.title);
    else
      return encodeURIComponent(document.title);
  }

  getUrl() {
    if (this.props.url)
      return encodeURIComponent(this.props.url);
    else
      return encodeURIComponent(String(window.location));
  }

  async getLinkedInShareCount() {
    try {
      const apiLink: string = `https://www.linkedin.com/countserv/count/share?url=${this.getUrl()}&format=json`;
      const response: Response = await fetch(apiLink);
      const parsed = await response.json();
      return parsed.count;
    } catch (e) {
      return 0;
    }
  }

  async getFacebookShareCount() {
    const apiLink: string = `https://graph.facebook.com/?id=${this.getUrl()}`;
    const response: Response = await fetch(apiLink);
    const parsed = await response.json();
    return parsed.share.share_count;
  }

  async getShareCount() {
    try {
      const shareCount: number = await this.getFacebookShareCount();
      this.setState({ shareCount, shareCountState: States.READY });
    } catch (e) {
      this.setState({ shareCountError: e, shareCountState: States.ERROR });
    }
  }

  getTwitterUrl () {
    return `https://twitter.com/intent/tweet?text=${this.getTitle()}&url=${this.getUrl()}`;
  }

  getFacebookUrl() {
    return `https://www.facebook.com/sharer/sharer.php?u=${this.getUrl()}`;
  }

  getLinkedInUrl () {
    return `https://www.linkedin.com/shareArticle?mini=true&url=${this.getUrl()}&title=${this.getTitle()}&source=${encodeURIComponent(window.location.origin)}`;
  }

  render() {
    return (
      <div className="share-buttons">
        <div className="row">
          <div className="col-xs-3 share-count">
            {this.state.shareCount}<br /> Shares
          </div>
          <div className="col-xs-3">
            <a href={this.getFacebookUrl()} target="_blank">
              <div className="social-media-share-buttons social-media-share-button-facebook">
                <FontAwesomeIcon icon={faFacebook} />
              </div>
            </a>
          </div>
          <div className="col-xs-3">
            <a href={this.getTwitterUrl()}>
              <div className="social-media-share-buttons social-media-share-button-twitter">
                <FontAwesomeIcon icon={faTwitter} />
              </div>
            </a>
          </div>
          <div className="col-xs-3">
            <a href={this.getLinkedInUrl()}>
              <div className="social-media-share-buttons social-media-share-button-linkedin">
                <FontAwesomeIcon icon={faLinkedin} />
              </div>
            </a>
          </div>
        </div>
      </div>
    );
  }
}

const FullShareButtons = () => {
  return (
    <div className="container">
      <div className="col-xs-12 col-md-8 col-md-offset-2 share-container">
        <h3 className="share-title">Vertel het verder</h3>
        <div className="row">
          <div className="col-xs-12 col-sm-6 col-sm-offset-3 share-buttons-container">
            <ShareButtons />
          </div>
        </div>
      </div>
    </div>
  );
};

renderComponentIf(
  <FullShareButtons />,
  document.getElementById("homepage-share-buttons")
);

renderComponentIf(
  <ShareButtons />,
  document.getElementById("social-list-component")
);

renderComponentIf(
  <ShareButtons />,
  document.getElementById("project-details-share-buttons")
);

renderComponentIf(
  <FullShareButtons />,
  document.getElementById("project-details-share-buttons-row")
);

renderComponentIf(
  <ShareButtons title={document.getElementById("project-details-share-buttons-custom") && document.getElementById("project-details-share-buttons-custom").dataset.title}
                url={document.getElementById("project-details-share-buttons-custom") && document.getElementById("project-details-share-buttons-custom").dataset.link} />,
  document.getElementById("project-details-share-buttons-custom")
);