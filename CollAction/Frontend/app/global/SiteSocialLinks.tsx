import * as React from "react";
import * as ReactDOM from "react-dom";
import renderComponentIf from "./renderComponentIf";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { library } from "@fortawesome/fontawesome-svg-core";
import { faTwitter } from "@fortawesome/free-brands-svg-icons";
import { faFacebook } from "@fortawesome/free-brands-svg-icons";
import { faLinkedin } from "@fortawesome/free-brands-svg-icons";
import { faYoutube } from "@fortawesome/free-brands-svg-icons";

library.add(faTwitter, faFacebook, faLinkedin, faYoutube);

class SiteSocialLinks extends React.Component {
  constructor(props) {
    super(props);
  }

  getTwitterUrl () {
    return `https://twitter.com/CollAction_org`;
  }

  getFacebookUrl() {
    return `https://www.facebook.com/collaction.org/`;
  }

  getLinkedInUrl () {
    return `https://www.linkedin.com/company/stichting-collaction/`;
  }

  getYoutubeUrl () {
    return `https://www.youtube.com/channel/UCC2SBF4mbeKXrHqnMuN6Iew/`;
  }

  render() {
    return (
        <ul className="social-list">
        <li>
            <a href={this.getFacebookUrl()} target="_blank">
              <div className="social-media-share-buttons social-media-share-button-facebook">
                <FontAwesomeIcon icon={faFacebook} />
              </div>
            </a>
        </li>
        <li>
            <a href={this.getTwitterUrl()}>
              <div className="social-media-share-buttons social-media-share-button-twitter">
                <FontAwesomeIcon icon={faTwitter} />
              </div>
            </a>
        </li>
        <li>
            <a href={this.getLinkedInUrl()}>
              <div className="social-media-share-buttons social-media-share-button-linkedin">
                <FontAwesomeIcon icon={faLinkedin} />
              </div>
            </a>
        </li>
        <li>
        <a href={this.getYoutubeUrl()}>
              <div className="social-media-share-buttons social-media-share-button-youtube">
                <FontAwesomeIcon icon={faYoutube} />
              </div>
            </a>
        </li>
      </ul>
    );
  }
}

renderComponentIf(
  <SiteSocialLinks />,
  document.getElementById("social-list-component")
);
