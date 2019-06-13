import * as React from "react";
import renderComponentIf from "./renderComponentIf";

interface ICookieMessageProps {
    canTrack: boolean;
    cookieString: string;
}

interface ICookieMessageState {
    showBanner: boolean;
}

export default class CookieMessage extends React.Component<ICookieMessageProps, ICookieMessageState> {
    constructor(props) {
        super(props);
        this.state = { showBanner: false };

        this.accept = this.accept.bind(this);
    }

    componentDidMount() {
        this.setState({
            showBanner: !this.props.canTrack
        });
    }

    accept() {
        document.cookie = this.props.cookieString;
        location.reload();
    }

    renderMessage() {
        return (
            <div>
                <div id="cookie-message-banner">
                    <div className="container">
                        <div className="row">
                            <div className="col-xs-12">
                                <p>Thank you for visiting our website! Please note that our website uses cookies to analyze and improve the performance of our website and to make social media integration possible. For more information on the use of cookies and privacy related matters, please see our <a href="/home/privacy">Privacy and Cookies Policy</a>. By clicking "I accept" or by using this site, you consent to the use of cookies.</p>
                                <p><a className="btn" href="#" onClick={this.accept}>Yes, I accept cookies</a></p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }

    render() {
        if (this.state.showBanner === true) {
            return this.renderMessage();
        }
        else {
            return (<div></div>);
        }
    }
}

renderComponentIf(
    <CookieMessage
        canTrack={ document.getElementById("cookie-message") && document.getElementById("cookie-message").dataset.canTrack === "True" }
        cookieString= { document.getElementById("cookie-message") && document.getElementById("cookie-message").dataset.cookieString }
    />,
    document.getElementById("cookie-message")
);
