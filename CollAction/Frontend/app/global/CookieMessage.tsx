import * as React from "react";
import renderComponentIf from "./renderComponentIf";

export default class CookieMessage extends React.Component {
    render() {
        return (
            <div>
                <div id="cookie-message-banner">
                    <div className="container">
                        <div className="row">
                            <div className="col-xs-12">
                                <p>Thank you for visiting our website! Please note that our website uses cookies to analyze and improve the performance of our website and to make social media integration possible. For more information on the use of cookies and privacy related matters, please see our <a href="home/privacy">Privacy and Cookies Policy</a>. By clicking "I accept" or by using this site, you consent to the use of cookies.</p>
                                <p><a className="btn" href="#">Yes, I accept cookies</a></p>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

renderComponentIf(
    <CookieMessage />,
    document.getElementById("cookie-message")
);
