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
};

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
};

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
    );
  }
}

const teamMembers: ITeamMember[] = [
  {
    name: "Abhinay Kondamreddy",
    role: "",
    photo: "NoPhoto.png"
  },
  {
    name: "Christa Brouwer",
    role: "",
    photo: "NoPhoto.png"
  },
  {
    name: "Daniela Becker",
    role: "",
    photo: "Daniela Becker.png"
  },
  {
    name: "Dominik Guz",
    role: "",
    photo: "NoPhoto.png"
  },
  {
    name: "Edoardo Felici",
    role: "",
    photo: "Edoardo Felici.png"
  },
  {
    name: "Luuk Boschker",
    role: "",
    photo: "Luuk Boschker.png"

  },
  {
    name: "Laura Wennekes",
    role: "",
    photo: "NoPhoto.png"
  },
  {
    name: "Martijn de Haan",
    role: "",
    photo: "Martijn de Haan.png"
  },
  {
    name: "Melchior Jong",
    role: "",
    photo: "Melchior Jong.png"
  },
  {
    name: "Mina Yao",
    role: "",
    photo: "NoPhoto.png"
  },
  {
    name: "Nikie van Thiel",
    role: "",
    photo: "NoPhoto.png"
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
    name: "Vivian Vos",
    role: "",
    photo: "Vivian Vos.png"
  },
  {
    name: "Brian Russell",
    role: "",
    photo: "NoPhoto.png"
  },
  {
    name:"Alexis Padula",
    role:"",
    photo:"NoPhoto.png"
  }
];

renderComponentIf(
  <MeetTheTeam teamMembers={teamMembers} />,
  document.getElementById("meet-the-team-widget")
);
