import renderComponentIf from "../global/renderComponentIf";
import * as React from "react";
import IdealBox from "./IdealBox";
import {StripeProvider, Elements, injectStripe} from 'react-stripe-elements';
import * as Modal from 'react-modal';

const InjectedIdealBox = injectStripe(IdealBox);

const popupStyles = {
  content : {
    top                   : '50%',
    left                  : '50%',
    right                 : 'auto',
    bottom                : 'auto',
    marginRight           : '-50%',
    transform             : 'translate(-50%, -50%)',
    backgroundColor       : '#efefef',
    overlay: {
       backgroundColor: '#efefef'
    }
  }
};


interface IDonationBoxProps {
    stripePublicKey: string;
    userEmail: string;
    userName: string;
}

interface IDonationBoxState {
    amount: number;
    showError: boolean;
    error: string;
    showDialog: boolean;
    inputUserName: string;
    inputUserEmail: string;
}

class DonationBox extends React.Component<IDonationBoxProps, IDonationBoxState> {
    constructor(props) {
        super(props);
        this.state = { 
            amount: 0, 
            showError: false, 
            error: "",
            showDialog: false,
            inputUserEmail: "",
            inputUserName: ""
        };
    }

    setAmount(event) {
        let newAmount = Number.parseInt(event.currentTarget.value);
        if (!newAmount) {
            newAmount = 0;
        }
        this.setState({amount: newAmount, showError: false});
    }

    clearErrors() {
        this.setState({ showError: false, error: null });
    }

    needsToEnterName() {
        return this.props.userName.match(/^ *$/) !== null;
    }

    needsToEnterEmail() {
        return this.props.userEmail.indexOf("@") < 0;
    }

    async payIdeal() {
        if (this.checkFormErrors()) {
            return;
        }
        this.setState({ showDialog: true });
    }

    checkFormErrors() {
        this.clearErrors();
        if (this.state.amount <= 0) {
            this.setState({ showError: true, error: "Please fill in a valid and positive donation amount" });
            return true;
        }

        if (this.getName() == "" || this.getEmail().indexOf("@") < 0) {
            this.setState({ showError: true, error: "Please fill in your user-details" });
            return true;
        }

        return false;
    }

    async payCreditCard() {
        if (this.checkFormErrors()) {
            return;
        }

        let checkoutTokenResponse: Response = await fetch(`/Donation/InitializeCreditCardCheckout?currency=eur&amount=${this.state.amount}&name=${encodeURIComponent(this.getName())}&email=${encodeURIComponent(this.getEmail())}`, { method: "POST" });
        if (checkoutTokenResponse.status != 200) {
            let responseBody = await checkoutTokenResponse.text();
            console.log("Unable to redirect to checkout: " + responseBody);
            this.setState({ showError: true, error: "Unable to initialize checkout" });
            return;
        }

        let options = {
          betas: ['checkout_beta_4']
        } as stripe.StripeOptions; // "betas" option is not supported, need to cast
        let stripe: any = Stripe(this.props.stripePublicKey, options); // cast to any because redirectToCheckout is a beta API
        let checkoutId = await checkoutTokenResponse.text();
        let checkoutResponse = await stripe.redirectToCheckout({ sessionId: checkoutId });
        if (checkoutResponse.status != 200) {
            let responseBody = await checkoutResponse.text();
            console.log("Unable to redirect to checkout: " + responseBody);
            this.setState({ showError: true, error: "Unable to redirect to checkout" });
        }
    }

    onCloseDialog() {
        this.setState({ showDialog: false });
    }

    setName(ev: React.ChangeEvent<HTMLInputElement>) {
        this.setState({ inputUserName: ev.currentTarget.value });
    }

    setEmail(ev: React.ChangeEvent<HTMLInputElement>) {
        this.setState({ inputUserEmail: ev.currentTarget.value });
    }

    getName() {
        if (!this.needsToEnterName()) {
            return this.props.userName;
        } else {
            return this.state.inputUserName;
        }
    }

    getEmail() {
        if (!this.needsToEnterEmail()) {
            return this.props.userEmail;
        } else {
            return this.state.inputUserEmail;
        }
    }

    renderAmount(amount: number) {
        const id = `amount${amount}`;
        return (
            <div className="col-xs-6 col-sm-3">
                <input id={id} type="radio" name="amount" value={amount} onChange={(event) => this.setAmount(event)} checked={this.state.amount === amount} />
                <label htmlFor={id}>&euro; {amount}</label>
            </div>
        );
    }

    renderCustomAmount() {
        return (
            <div className="col-xs-6 col-sm-3">
                <input type="text" id="amountCustom" name="amount" placeholder="Other..." onChange={(event) => this.setAmount(event)} />
            </div>
        );
    }

    renderName() {
        if (this.needsToEnterName()) {
            return (
                <div id="name-input-box">
                    <label htmlFor="name-input">Name</label>
                    <input id="name-input" className="form-control" onChange={(ev) => this.setName(ev)} type="text" />
                </div>);
        }
    }

    renderEmail() {
        if (this.needsToEnterEmail()) {
            return (
                <div id="email-input-box">
                    <label htmlFor="email-input">E-Mail</label>
                    <input id="email-input" className="form-control" onChange={(ev) => this.setEmail(ev)} type="text" />
                </div>);
        }
    }

    renderUserDetails() {
        if (this.needsToEnterEmail() || this.needsToEnterName()) {
            return (
                <div className="user-details">
                    {this.renderName()}
                    {this.renderEmail()}
                </div>);
        }
    }

    renderAmounts() {
        return (
            <div className="row">
                {this.renderAmount(3)}
                {this.renderAmount(5)}
                {this.renderAmount(10)}
                {this.renderAmount(20)}
                {this.renderAmount(30)}
                {this.renderAmount(50)}
                {this.renderAmount(100)}
                {this.renderCustomAmount()}
            </div>);
    }

    renderPopup() {
        return (
            <Modal isOpen={this.state.showDialog} onRequestClose={() => this.onCloseDialog()} contentLabel="Donate" style={popupStyles}>
                <div id="donation-modal">
                    <StripeProvider apiKey={this.props.stripePublicKey}>
                        <Elements>
                            <InjectedIdealBox amount={this.state.amount} userEmail={this.getEmail()} userName={this.getName()} />
                        </Elements>
                    </StripeProvider>
                    <a className="btn" onClick={() => this.onCloseDialog()}>Close</a>
                </div>
            </Modal>);
    }

    renderPaymentOptions() {
        return (<div className="payment-options">
            <div className="row">
                <div className="col-xs-12">
                    <button className="btn" value="ideal" onClick={() => this.payIdeal()}>
                        <img src="/images/thankyoucommit/iDEAL-logo.png" />
                        <div>iDEAL</div>
                    </button>
                </div>
            </div>
            <div className="row">
                <div className="col-xs-12">
                    <button className="btn" value="creditcard" onClick={() => this.payCreditCard()}>
                        <img src="/images/thankyoucommit/bank-card.png" />
                        <div>Debit / Creditcard</div>
                    </button>
                </div>
            </div>
        </div>);
    }

    renderErrors() {
        return (
            <div className="error" hidden={!this.state.showError}>
                <p>{this.state.error}</p>
            </div>);
    }

    renderHeader() {
        return (
            <div>
                <h2>Help us reaching our mission by donating!</h2>
                <p>
                    By donating to CollAction you help us solve collactive action problems and to make the world a better place. Your donation helps us maintaining and growing the platform and ultimately multiply our impact.
                </p>
                <hr />
                <p>
                    If you'd like make a one-time donation, please select the amount and payment option.
                </p>
            </div>);
    }

    render() {
        return (
            <div className="donation-box">
                {this.renderHeader()}
                {this.renderErrors()}
                {this.renderUserDetails()}
                {this.renderAmounts()}
                {this.renderPopup()}
                {this.renderPaymentOptions()}
            </div>
        );
    }
}

let donationBox: HTMLElement = document.getElementById("donation-box");
if (donationBox !== null) {
    let stripePublicKey = donationBox.getAttribute("data-stripe-public-key");
    let name = donationBox.getAttribute("data-user-name");
    let email = donationBox.getAttribute("data-user-email");
    renderComponentIf(
        <DonationBox stripePublicKey={stripePublicKey} userName={name} userEmail={email} />,
        donationBox
    );
}