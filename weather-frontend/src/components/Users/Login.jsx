import axios from "axios";
import React, { useState, useContext } from "react";
import { Button, Form, Modal, Nav } from "rsuite";
import { UserApi } from "../../configuration/API";
import { AppContext } from "../Auth/AppProvider";

export const MenuLogin = () => {
  const { dispatch } = useContext(AppContext);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [visible, setVisible] = useState(false);

  const signInAction = () => {
    axios
      .post(
        UserApi.login,
        {
          email: email,
          password: password,
        },
        {
          headers: {
            "Content-Type": "application/json",
            "Access-Control-Allow-Origin": "http://localhost:3000",
          },
        }
      )
      .then((response) => {
        console.log(response.data);
        dispatch({
          type: "USER_FOUND",
          payload: {
            user: response.data.user,
            token: response.data.token,
          },
        });
      })
      .catch((error) => {
        //console.error(error);
      });
  };

  return (
    <>
      <Nav.Item
        onClick={(e) => {
          setVisible(!visible);
        }}
      >
        Přihlásit se
      </Nav.Item>

      <Modal
        title="Přihlášení"
        open={visible}
        backdrop={true}
        onClose={() => setVisible(false)}
        keyboard={true}
      >
        <Modal.Header>
          <Modal.Title>Přihlášení</Modal.Title>
        </Modal.Header>
        <Modal.Body>
          <Form>
            <Form.Group controlId="email">
              <Form.ControlLabel>Email</Form.ControlLabel>
              <Form.Control
                type="email"
                placeholder="Email"
                name="email"
                value={email}
                onChange={(e) => setEmail(e)}
              />
              <Form.HelpText tooltip>Required</Form.HelpText>
            </Form.Group>
            <Form.Group controlId="password">
              <Form.ControlLabel>Heslo</Form.ControlLabel>
              <Form.Control
                type="password"
                placeholder="Heslo"
                name="password"
                value={password}
                onChange={(e) => setPassword(e)}
              />
              <Form.HelpText tooltip>Required</Form.HelpText>
            </Form.Group>
          </Form>
        </Modal.Body>
        <Modal.Footer>
          <Button onClick={() => setVisible(false)} appearance="subtle">
            Zavřít
          </Button>
          <Button onClick={() => signInAction()} appearance="primary">
            Přihlásit se
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
};
