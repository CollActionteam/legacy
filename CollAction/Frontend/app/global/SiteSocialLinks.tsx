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

  componentDidMount() {
  }

  getTwitterUrl () {
    return `https://twitter.com/freonen`;
  }

  getFacebookUrl() {
    return `https://www.facebook.com/freonen/`;
  }

  getLinkedInUrl () {
    return `https://www.linkedin.com/company/freonen-fan-fossylfry-frysl%C3%A2n/`;
  }

  getYoutubeUrl () {
    return `https://www.youtube.com/channel/UCd8GD6UcaH8MEpJQhB3qzzA`;
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
