import * as React from "react";
import renderComponentIf from "../global/renderComponentIf";

interface ITeamMember {
  name: string;
  role: string;
  photo: string;
}

interface IMeetTheTeamProps {
  teamMembers: ITeamMember[];
}

interface IMeetTheTeamState {
  currentIndex: number;
}

const TeamMemberThumb = ({ teamMember }) => {
  const imageSrc = `/images/team/${teamMember.photo}`;
  return (
    <div className="team-member">
      <div className="team-member-image-container">
        <img src={imageSrc} />
      </div>
      <div>
        {teamMember.name}
      </div>
      <div>
        {teamMember.role}
      </div>
    </div>
  );
}

const NavigationDots = ({ currentIndex, length}) => {
  const dotClicked = () => {
    const node = this.getDomNode();
  };


  const dots = [].map((index) => {
    if (index === currentIndex) {
      return <div className="dot dot-active" key={index} onClick={dotClicked} />;
    }
    return <div className="dot" key={index} onClick={dotClicked}/>;
  });

  return (
    <div id="dot-container">
      { dots }
    </div>
  );
}

export default class MeetTheTeam extends React.Component<IMeetTheTeamProps, IMeetTheTeamState> {

  constructor (props) {
    super(props);
    this.state = { currentIndex: 0 };
  }

  render () {
    return (
      <div>
        <div id="team-member-container">
          {this.props.teamMembers.map(teamMember => <TeamMemberThumb key={teamMember.name} teamMember={teamMember} />)}
        </div>
        <NavigationDots currentIndex={this.state.currentIndex} length={this.props.teamMembers.length}/>
      </div>
    )
  }
}

const teamMembers: ITeamMember[] = [
  {
    name: "Barbera Putman Cramer",
    role: "",
    photo: "Barbara Putman Cramer.png"
  },
  {
    name: "Chris Schuchmann",
    role: "",
    photo: "Chris Schuchmann.png"
  },
  {
    name: "Daniela Becker",
    role: "",
    photo: "Daniela Becker.png"
  },
  {
    name: "Dominik Guz",
    role: "",
    photo: "Dominik Guz.png"
  },
  {
    name: "Edoardo Felici",
    role: "",
    photo: "Edoardo Felici.png"
  },
  {
    name: "Juan Naputi",
    role: "",
    photo: "Juan Naputi.png"
  },
  {
    name: "Laura Wennekes",
    role: "",
    photo: "Laura Wennekes.png"
  },
  {
    name: "Lena Hartog",
    role: "",
    photo: "Lena Hartog.png"
  },
  {
    name: "Luc Geurts",
    role: "",
    photo: "Luc Geurts.png"
  },
  {
    name: "Luuk Boschker",
    role: "",
    photo: "Luuk Boschker.png"
  },
  {
    name: "Nikie van Thiel",
    role: "",
    photo: "Nikie van Thiel.png"
  },
  {
    name: "Ron van den Akker",
    role: "",
    photo: "Ron van den Akker.png"
  },
  {
    name: "Spencer Heijnen",
    role: "",
    photo: "Spencer Heijnen.png"
  },
  {
    name: "Tim Stokman",
    role: "",
    photo: "Tim Stokman.png"
  },
  {
    name: "Tuanh Lam",
    role: "",
    photo: "Tuanh Lam.png"
  },
  {
    name: "Vivian Vos",
    role: "",
    photo: "Vivian Vos.png"
  },
  {
    name: "Brian Russell",
    role: "",
    photo: "Brian Russell.png"
  },
];

renderComponentIf(
  <MeetTheTeam teamMembers={teamMembers} />, 
  document.getElementById("meet-the-team-widget")
);
