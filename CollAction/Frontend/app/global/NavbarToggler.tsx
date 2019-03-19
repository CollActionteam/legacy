import * as React from "react";
import renderComponentIf from "./renderComponentIf";
//import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
//import { faTimes } from "@fortawesome/free-solid-svg-icons";
//import { faBars } from "@fortawesome/free-solid-svg-icons";

interface INavbarTogglerProps {
    targetId: string;
    toggleClass: string;
}

interface INavbarToggerState {
    open: boolean;
}

export default class NavbarToggler extends React.Component<INavbarTogglerProps, INavbarToggerState> {
    private target: HTMLElement;

    constructor (props?: INavbarTogglerProps) {
        super(props);
        this.state = {
            open: false
        };

        this.toggle = this.toggle.bind(this);

        this.target = document.getElementById(this.props.targetId);
        if (this.target === undefined) console.error(`Can't link navbar-toggler to ${this.props.targetId}.`);
    }

    toggle() {
        const newState = !this.state.open;
        this.setState({
            open: newState
        });

        if (newState) {
            this.target.classList.add(this.props.toggleClass);
        }
        else {
            this.target.classList.remove(this.props.toggleClass);
        }
    }

    render() {
        return (
            <button onClick={this.toggle}>
            </button>);
        //return (
            //<button onClick={this.toggle}>
                //<FontAwesomeIcon icon={this.state.open ? faTimes : faBars } />
            //</button>
        //);
    }
}

const toggler = document.getElementById("navbar-toggler");
renderComponentIf(
    <NavbarToggler
        targetId={toggler && toggler.dataset.targetId}
        toggleClass={toggler && toggler.dataset.toggleClass}
    />,
    toggler
);
