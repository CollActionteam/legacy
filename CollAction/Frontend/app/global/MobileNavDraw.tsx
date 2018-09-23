import * as React from "react";
import renderComponentIf from "./renderComponentIf";
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome'
import { faTimes } from '@fortawesome/free-solid-svg-icons'
import { faBars } from '@fortawesome/free-solid-svg-icons'

interface IMobileNavDrawProps {
  loginText?: string;
}

interface IMobileNavDrawState {
  open: boolean;
}

export default class MobileNavDraw extends React.Component<IMobileNavDrawProps, IMobileNavDrawState> {

  constructor (props?: IMobileNavDrawProps, context?: any) {
    super(props, context);
    this.state = { open: false };
  }

  open() {
    this.setState({open: true});
  }

  close() {
    this.setState({open: false});
  }

  renderMenu () {
    const self = this;
    return(
      <div id="draw-menu">
        <div id="draw-menu-close-button" onClick={() => self.close()}>
          <FontAwesomeIcon icon="faTimes" />
        </div>
        <ul>
            <li><a href="/">Home</a></li>
            <li><a href="/find">Vind Project</a></li>
            <li><a href="/start">Start Project</a></li>
            <li><a href="/about">Over</a></li>
            {this.props.loginText == "Logout" ?
                (<li><button type="button" onClick={() => document.getElementById("logOutBtn").click()}>Logout</button></li>) :
                (<li><a href="/account/login">Login</a></li>)
            }
        </ul>
      </div>
    );
  }

  render () {
    const self = this;
    return (
      <div className="mobile-header">
        <i className="fa fa-bars"
          id="hamburger-icon"
          aria-hidden="true"
          onClick={() => self.open()}>
        </i>
        { this.state.open ? this.renderMenu() : null }
      </div>
    );
  }
}

var loginText = document.getElementById("logOutBtn")? "Logout" : "Login";

renderComponentIf(
  <MobileNavDraw loginText={loginText} />,
  document.getElementById("mobile-nav-draw")
);
