import renderComponentIf from "../global/renderComponentIf";
import * as React from "react";
import BankDetailsBox from "./BankDetailsBox";
import {StripeProvider, Elements, injectStripe} from 'react-stripe-elements';
import * as Modal from 'react-modal';

const InjectedBankDetailsBox = injectStripe(BankDetailsBox);

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
    showBankDialog: boolean;
    inputUserName: string;
    inputUserEmail: string;
    isRecurring: boolean;
}

class DonationBox extends React.Component<IDonationBoxProps, IDonationBoxState> {
    constructor(props) {
        super(props);
        this.state = { 
            amount: 0, 
            showError: false, 
            error: "",
            showBankDialog: false,
            isRecurring: false,
            inputUserEmail: "",
            inputUserName: ""
        };
    }

    clearErrors() {
        this.setState({ showError: false, error: null });
    }

    needsToEnterName(): boolean {
        return this.props.userName === "";
    }

    needsToEnterEmail(): boolean {
        return this.props.userEmail.match(/.+@.+/) === null;
    }

    checkFormErrors() {
        this.clearErrors();
        if (this.state.amount <= 0) {
            this.setState({ showError: true, error: "Please fill in a valid and positive donation amount" });
            return true;
        }

        if (this.getName() == "" || this.getEmail().match(/.+@.+/) === null) {
            this.setState({ showError: true, error: "Please fill in your user-details" });
            return true;
        }

        return false;
    }

    async payBank() {
        if (this.checkFormErrors()) {
            return;
        }
        this.setState({ showBankDialog: true });
    }

    async payCreditCard() {
        if (this.checkFormErrors()) {
            return;
        }

        let checkoutTokenUrl = `/Donation/InitializeCreditCardCheckout?currency=eur&amount=${this.state.amount}&name=${encodeURIComponent(this.getName())}&email=${encodeURIComponent(this.getEmail())}&recurring=${this.state.isRecurring}`;
        let checkoutTokenResponse: Response = await fetch(checkoutTokenUrl, { method: "POST" });
        if (checkoutTokenResponse.status != 200) {
            let responseBody = await checkoutTokenResponse.text();
            console.log("Unable to redirect to checkout: " + responseBody);
            this.setState({ showError: true, error: "Unable to initialize checkout" });
            return;
        }

        let stripe: any = Stripe(this.props.stripePublicKey); // cast to any because redirectToCheckout is not yet in stripe.js
        let checkoutId = await checkoutTokenResponse.text();
        let checkoutResponse = await stripe.redirectToCheckout({ sessionId: checkoutId });
        if (checkoutResponse.status != 200) {
            let responseBody = await checkoutResponse.text();
            console.log("Unable to redirect to checkout: " + responseBody);
            this.setState({ showError: true, error: "Unable to redirect to checkout" });
        }
    }

    onCloseDialog() {
        this.setState({ showBankDialog: false });
    }

    setName(ev: React.ChangeEvent<HTMLInputElement>) {
        this.setState({ inputUserName: ev.currentTarget.value });
    }

    setEmail(ev: React.ChangeEvent<HTMLInputElement>) {
        this.setState({ inputUserEmail: ev.currentTarget.value });
    }

    setAmount(event: React.ChangeEvent<HTMLInputElement>) {
        let newAmount = Number.parseInt(event.currentTarget.value);
        if (!newAmount) {
            newAmount = 0;
        }
        this.setState({amount: newAmount, showError: false});
    }
    
    setIsOneOff(event: React.ChangeEvent<HTMLInputElement>) {
        this.setState({ isRecurring: !event.currentTarget.checked });
    }

    setIsMonthly(event: React.ChangeEvent<HTMLInputElement>) {
        this.setState({ isRecurring: event.currentTarget.checked });
    }

    getName(): string {
        if (!this.needsToEnterName()) {
            return this.props.userName;
        } else {
            return this.state.inputUserName;
        }
    }

    getEmail(): string {
        if (!this.needsToEnterEmail()) {
            return this.props.userEmail;
        } else {
            return this.state.inputUserEmail;
        }
    }

    renderPeriodSelection() {
        return (
            <React.Fragment>
                <div className="col-xs-12 col-sm-6 donation-period-one-off">
                    <input id="one-off-donation-button" type="radio" name="period" value="one-off" onChange={(event) => this.setIsOneOff(event)} checked={!this.state.isRecurring} />
                    <label htmlFor="one-off-donation-button">One-off</label>
                </div>
                <div className="col-xs-12 col-sm-6 donation-period-monthly">
                    <input id="monthly-donation-button" type="radio" name="period" value="one-off" onChange={(event) => this.setIsMonthly(event)} checked={!!this.state.isRecurring} />
                    <label htmlFor="monthly-donation-button">Monthly</label>
                </div>
            </React.Fragment>
        );
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
                    <input id="name-input" className="form-control" onChange={(ev) => this.setName(ev)} placeholder="Your name..." type="text" />
                </div>);
        }
    }

    renderEmail() {
        if (this.needsToEnterEmail()) {
            return (
                <div id="email-input-box">
                    <label htmlFor="email-input">E-Mail</label>
                    <input id="email-input" className="form-control" onChange={(ev) => this.setEmail(ev)} placeholder="Your e-mail..." type="text" />
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
            <Modal isOpen={this.state.showBankDialog} onRequestClose={() => this.onCloseDialog()} contentLabel="Donate" style={popupStyles}>
                <div id="donation-modal">
                    <StripeProvider apiKey={this.props.stripePublicKey}>
                        <Elements>
                            <InjectedBankDetailsBox amount={this.state.amount} userEmail={this.getEmail()} userName={this.getName()} isRecurring={this.state.isRecurring} />
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
                    <button className="btn" value="ideal" onClick={() => this.payBank()}>
                        <img src="/images/thankyoucommit/iDEAL-logo.png" />
                        <div>iDeal / SEPA Direct</div>
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
                {this.renderPeriodSelection()}
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